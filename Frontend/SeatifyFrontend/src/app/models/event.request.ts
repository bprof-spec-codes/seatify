export default interface EventRequest {
  organizerId: string;
  slug: string;
  name: string;
  description: string;
  status: string;
  currency?: string;
}
