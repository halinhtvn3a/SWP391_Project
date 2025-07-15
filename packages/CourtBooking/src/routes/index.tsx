import { createFileRoute } from '@tanstack/react-router';
import Login from '../features/auth/components/login';

export const Route = createFileRoute('/')({
  component: Index,
});

function Index() {
  return <div><Login/></div>;
}
