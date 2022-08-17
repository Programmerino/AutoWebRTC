# AutoWebRTC

## Build
This project uses [Nix](https://nixos.org/download.html) and [Nix Flakes](https://nixos.wiki/wiki/Flakes#Permanent) for the building process. Once installed, `cachix use programmerino` can be used to speed up the initial compile once [Cachix](https://github.com/cachix/cachix) is installed.

Place binaries for ``peerconnection_client`` and ``peerconnection_server`` at the root of the project and run ``nix build -L`` to create the binary at ``result/bin/app``

## Example Usage
The server and clients must be running SSH servers and registered with the computer running the application
``./result/bin/app --privateKey "$(cat ~/.ssh/id_ed25519)" --server "serveruser@serverhost" --firstnode "user1@host1" --secondnode "user2@host2"``