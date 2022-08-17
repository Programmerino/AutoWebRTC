const  { nativeNodeModulesPlugin }  = require("esbuild-native-node-modules-plugin");

require('esbuild').build({
    entryPoints: ["AutoWebRTC.fs.js"],
    bundle: true,
    outdir: "built",
    plugins: [nativeNodeModulesPlugin],
    platform: 'node',
    target: 'esnext',
    treeShaking: true
}).catch(() => process.exit(1))