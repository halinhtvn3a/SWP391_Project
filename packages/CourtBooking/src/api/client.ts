import createClient from 'openapi-fetch';
import type { Middleware } from 'openapi-fetch';
import type { paths } from './types.ts';

const githubToken = import.meta.env.VITE_GITHUB_TOKEN;
const baseUrl = 'https://api.github.com';

const authMiddleware: Middleware = {
  onRequest: async ({ request }) => {
    request.headers.set('Authorization', `Bearer ${githubToken}`);
    return request;
  },
};

const client = createClient<paths>({ baseUrl });

client.use(authMiddleware);

// const baseUrl = 'https://localhost:7104/';

// const client = createClient<paths>({ baseUrl });

export { client };
