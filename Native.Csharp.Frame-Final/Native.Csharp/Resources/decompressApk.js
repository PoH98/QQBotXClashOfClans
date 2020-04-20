const { readdirSync, readFileSync, writeFileSync, lstatSync } = require('fs');
const { resolve } = require('path');
const readline = require('readline');
const ScCompression = require('./sc-compression');

const rl = readline.createInterface({
    input: process.stdin,
    output: process.stdout
});
#PATH#
console.log(`Decompressing files in ${anyPath}...`);
readdirSync(anyPath).forEach((file) => {
    const filepath = resolve(anyPath, file);
    const buffer = readFileSync(filepath);
    writeFileSync(filepath, ScCompression.decompress(buffer));
});
