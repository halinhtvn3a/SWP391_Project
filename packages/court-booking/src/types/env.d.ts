/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly VITE_OPENAPI_URL: string;
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}
