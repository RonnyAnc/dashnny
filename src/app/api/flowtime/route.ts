import { Deta } from "deta";
import { BasicType } from "deta/dist/types/types/basic";
import { v4 } from "uuid";

export class Flowtime {
	[id: string]: BasicType | null;
	task: string;
	id: string;
	startTime: string;
	stopTime: string | null;
	interruptions: number;

	constructor(
		id: string,
		task: string,
		startTime: string,
		stopTime: string | null = null,
		interruptions = 0
	) {
		this.id = id;
		this.task = task;
		this.startTime = startTime;
		this.stopTime = stopTime;
		this.interruptions = interruptions;
	}
}

export interface StartFlowtimeRequest {
	startTime: string;
	task: string;
}

export async function POST(request: Request): Promise<Response> {
	const body = JSON.parse(await request.text()) as StartFlowtimeRequest;
	const deta = Deta();
	const db = deta.Base("flowtimes");
	const flowtime = new Flowtime(v4(), body.task, body.startTime);
	await db.insert(flowtime, flowtime.id);

	return await Promise.resolve(new Response(JSON.stringify(flowtime)));
}

export interface StopFlowtimeRequest {
	id: string;
	task: string;
	startTime: string;
	stopTime: string | null;
	interruptions: number;
}

export async function PUT(request: Request): Promise<Response> {
	const body = JSON.parse(await request.text()) as StopFlowtimeRequest;
	const deta = Deta();
	const db = deta.Base("flowtimes");
	const flowtime = new Flowtime(
		body.id,
		body.task,
		body.startTime,
		body.stopTime,
		body.interruptions
	);
	await db.update(flowtime, flowtime.id);

	return await Promise.resolve(new Response(JSON.stringify(flowtime)));
}
