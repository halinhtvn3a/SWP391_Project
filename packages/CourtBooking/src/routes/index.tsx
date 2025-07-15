import { createFileRoute } from '@tanstack/react-router';
import { useGetQuery } from '../api/hook';
export const Route = createFileRoute('/')({
  component: Home,
});

function Home() {
  const booking = useGetQuery('/api/Bookings/current-time', {
    baseUrl: '/api',
  });
  console.log('Current Booking Time:', booking);
  return (
    <div className='p-2'>
      <h3>Welcome Home!!!</h3>
    </div>
  );
}
