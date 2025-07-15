/// <reference types="vite/client" />

import {
  Outlet,
  createRootRoute,
  HeadContent,
  Scripts,
} from '@tanstack/react-router';
// import '../styles/app.css';

import appCss from '../../styles/app.css?url';
import type { ReactNode } from 'react';
import { GoogleOAuthProvider } from '@react-oauth/google';

export const Route = createRootRoute({
  head: () => ({
    meta: [
      {
        charSet: 'utf-8',
      },
      {
        name: 'viewport',
        content: 'width=device-width, initial-scale=1',
      },
      {
        title: 'TanStack Start Starter',
      },
    ],
    links: [{ rel: 'stylesheet', href: appCss }],
  }),
  component: RootComponent,
});

function RootComponent() {
  return (
    <RootDocument>
      <Outlet />
    </RootDocument>
  );
}

function RootDocument({ children }: Readonly<{ children: ReactNode }>) {
  return (
    <html>
      <head>
        <HeadContent />
      </head>
      <body>
              <GoogleOAuthProvider clientId="">

        {children}
              </GoogleOAuthProvider>
        <Scripts />
      </body>
    </html>
  );
}
