export async function GET(_: Request): Promise<Response> {
	return await Promise.resolve(new Response("Hello, Next.js!"));
}
