import { defineConfig } from 'vite';
// import react from '@vitejs/plugin-react';
import tsConfigPaths from 'vite-tsconfig-paths';
import { tanstackStart } from '@tanstack/react-start/plugin/vite';
import tailwindcss from '@tailwindcss/vite';
import { tanstackRouter } from '@tanstack/router-plugin/vite';

import fs from 'fs';
import path from 'path';

export default defineConfig({
  server: {
    port: 3000,
    proxy: {},
    https: {
      key: fs.readFileSync(path.resolve(__dirname, 'certs/key.pem')),
      cert: fs.readFileSync(path.resolve(__dirname, 'certs/cert.pem')),
    },
  },
  plugins: [
    tanstackRouter({
      target: 'react',
      autoCodeSplitting: true,
    }),
    // react({
    //   jsxRuntime: 'automatic',
    //   babel: {},
    // }),
    tsConfigPaths({
      projects: ['./tsconfig.json'],
    }),
    tanstackStart({
      // customViteReactPlugin: true,
    }),
    tailwindcss(),
  ],
});
