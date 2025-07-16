import { createFileRoute } from '@tanstack/react-router';
import { useGetQuery } from '../api/hook';
import { useMemo } from 'react';
export const Route = createFileRoute('/')({
  component: Home,
});

function Home() {
  console.log('Home Component Rendered');
  const gists = useGetQuery('/gists', { params: { query: { per_page: 5 } } });
  // console.log('Current gist Time:', gists.error);
  if (gists.isLoading) {
    return <div className='p-2'>{gists.status}</div>;
  }

  return (
    <div className='p-2'>
      {/* {gists.data?.map((gist) => (
        <li key={gist.id}>
          <strong>{gist.description || 'Untitled'}</strong>
          <small>{new Date(gist.created_at).toLocaleTimeString()}</small>
        </li>
      ))} */}
      <h3>Welcome Home!!!</h3>
    </div>
  );
}
