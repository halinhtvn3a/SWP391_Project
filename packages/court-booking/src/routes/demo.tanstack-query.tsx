import { useGetQuery } from '@/api/base/hook'
import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute('/demo/tanstack-query')({
  component: TanStackQueryDemo,
})

function TanStackQueryDemo() {
  const booking = useGetQuery('/api/Bookings/current-time', {})
  if (booking.isLoading) {
    return <div className="p-4">Loading...</div>
  }

  console.log('booking', booking.data.data)

  return (
    <div className="p-4">
      <h1 className="text-2xl mb-4">Test open api + tanstack query</h1>
      <div>UTC Time: {booking.data.data.utcTime}</div>
      <div>Local Time: {booking.data.data.localTime}</div>
      <div>Time Zone: {booking.data.data.timeZone}</div>{' '}
    </div>
  )
}
