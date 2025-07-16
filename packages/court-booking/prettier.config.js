//  @ts-check

/** @type {import('prettier').Config} */
const config = {
  semi: false,
  singleQuote: true,
  tabWidth: 2,
  printWidth: 100,
  trailingComma: "none",
  jsxSingleQuote: true,
  plugins: ['prettier-plugin-tailwindcss']
};

export default config;
