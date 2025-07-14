import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/')({
  component: Index,
});

function Index() {
  return <div className='bg-red-500 text-white p-4'>Hello World</div>;
}
