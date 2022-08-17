{
  description = "AutoWebRTC";
  inputs.nixpkgs.url = "github:nixos/nixpkgs";
  inputs.flake-utils.url = "github:numtide/flake-utils";
  inputs.flake-compat = {
    url = github:edolstra/flake-compat;
    flake = false;
  };
  inputs.bundler.url = "github:NixOS/bundlers";
  inputs.fableFlake.url = "github:Programmerino/fable.nix";

  outputs = {
    self,
    nixpkgs,
    flake-utils,
    flake-compat,
    bundler,
    fableFlake,
  }:
    flake-utils.lib.eachDefaultSystem (
      system: let
        nodeSystem = "x86_64-linux";

        fable = fableFlake.defaultPackage."${system}";
        pkgs = import nixpkgs {
          inherit system;
        };

        targetPkgs = import nixpkgs {
          system = nodeSystem;
          config.allowUnfree = true;
        };

        name = "AutoWebRTC";
        version = let
          _ver = builtins.getEnv "GITVERSION_SEMVER";
        in
          if _ver == ""
          then "0.0.0"
          else "${_ver}";

        sdk = pkgs.dotnet-sdk;

        webrtcSrc = pkgs.fetchgit {
          url = "https://webrtc.googlesource.com/src.git";
          fetchSubmodules = true;
          rev = "67f80cea66cc84cf4ec7980694c5dc04ce7c4556";
          sha256 = "sha256-cXBJc/P2jTse7sXs/9sY+haz6hcdOEj85iSBBtIPdKg=";
        };

        peerconnection_client_unwrapped = targetPkgs.stdenv.mkDerivation rec {
          pname = "peerconnection_client";
          version = "0.0.0";

          dontUnpack = true;

          buildInputs = with targetPkgs; [
            xorg.libX11
            glib
            gtk3-x11
            cairo
          ];

          nativeBuildInputs = [
            targetPkgs.autoPatchelfHook
          ];

          runtimeDependencies = with targetPkgs; [
            libpressureaudio
          ];

          installPhase = ''
            install -m755 -D ${./peerconnection_client} $out/bin/peerconnection_client
          '';
        };

        peerconnection_server_unwrapped = targetPkgs.stdenv.mkDerivation {
          pname = "peerconnection_server";
          version = "0.0.0";

          dontUnpack = true;

          nativeBuildInputs = [
            targetPkgs.autoPatchelfHook
          ];

          buildInputs = with targetPkgs; [
          ];

          installPhase = ''
            install -m755 -D ${./peerconnection_server} $out/bin/peerconnection_server
          '';
        };

        peerconnection_server = targetPkgs.writeShellApplication {
          name = "peerconnection_server";
          text = ''
            ${peerconnection_server_unwrapped}/bin/peerconnection_server "$@"
          '';
        };

        cage = targetPkgs.cage.override {
          xwayland = null;
        };

        peerconnection_client = targetPkgs.writeShellApplication {
          name = "peerconnection_client";
          text = ''
            export WLR_BACKENDS=headless
            export XDG_RUNTIME_DIR=/run/user/$UID
            export GDK_BACKEND=wayland
            export WLR_RENDERER=pixman

            if ${targetPkgs.toybox}/bin/pgrep -x "pipewire" > /dev/null
            then
                echo "pipewire is already running"
            else
                ${targetPkgs.pipewire}/bin/pipewire &
            fi
            if ${targetPkgs.toybox}/bin/pgrep -x "pipewire-pulse" > /dev/null
            then
                echo "pipewire-pulse is already running"
            else
                ${targetPkgs.pipewire.pulse}/bin/pipewire-pulse &
            fi
            if ${targetPkgs.toybox}/bin/pgrep -x "wireplumber" > /dev/null
            then
                echo "wireplumber is already running"
            else
                ${targetPkgs.wireplumber}/bin/wireplumber &
            fi

            for (( i=1; i<=$1; i++ ))
            do
              echo "Starting instance $i..."
              ${cage}/bin/cage -d -- ${peerconnection_client_unwrapped}/bin/peerconnection_client "''${@:2}" &
            done
            wait
          '';
        };

        bundle = bundler.bundlers."${nodeSystem}".toArx;

        app = pkgs.writeShellApplication {
          name = "app";
          text = ''
            ${pkgs.nodejs-18_x}/bin/node ${./src/built}/AutoWebRTC.fs.js --serverPath "${bundle peerconnection_server}" --clientPath "${bundle peerconnection_client}" "$@"
          '';
        };
      in rec {
        devShells.default = pkgs.mkShell {
          inherit name;
          inherit version;
          doCheck = false;
          DOTNET_CLI_TELEMETRY_OPTOUT = 1;
          CLR_OPENSSL_VERSION_OVERRIDE = 1.1;
          DOTNET_SYSTEM_GLOBALIZATION_INVARIANT = 1;
          DOTNET_CLI_HOME = "/tmp/dotnet_cli";
          DOTNET_ROOT = "${sdk}";
          buildInputs = [pkgs.yarn sdk fable pkgs.nodejs-18_x pkgs.dotnetPackages.Paket pkgs.nix];
          shellHook = ''
            eval "$(starship init bash)"
          '';
        };

        # Necessary for flake-compat
        devShell = devShells.default;

        packages."${name}" = app;
        defaultPackage = packages."${name}";
        apps."${name}" = flake-utils.lib.mkApp {drv = app;};
        apps.build = flake-utils.lib.mkApp {
          drv = pkgs.writeShellApplication {
            name = "build";
            runtimeInputs = devShell.buildInputs;
            text = ''
              cd src
              touch AutoWebRTC.fs
              fable precompile lib/lib.fsproj -o lib/precompiled
              fable AutoWebRTC.fsproj --precompiledLib lib/precompiled
              node esbuild.cjs
              git add built
              cd ..
              nix build -L
              rm -rf src/built
            '';
          };
        };
        apps.default = apps."${name}";
      }
    );
}
