export async function GET(request: Request): Promise<Response> {
	return await Promise.resolve(new Response("Hello, Next.js!"));
}
