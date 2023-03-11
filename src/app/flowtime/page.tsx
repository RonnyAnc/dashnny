"use client";

import { useState } from "react";

import { Flowtime, StartFlowtimeRequest, StopFlowtimeRequest } from "../api/flowtime/route";

export default function FlowtimePage() {
	const emptyFlowtime = {
		task: "",
		id: "",
		startTime: "",
		stopTime: "",
		interruptions: 0,
	};
	const [flowtime, setFlowtime] = useState<Flowtime>(emptyFlowtime);
	function startWorking(event: React.FormEvent<HTMLFormElement>) {
		event.preventDefault();
		const startFlow: StartFlowtimeRequest = {
			startTime: new Date().toISOString(),
			task: flowtime.task,
		};
		void fetch("api/flowtime", {
			method: "POST",
			body: JSON.stringify(startFlow), // data can be `string` or {object}!
			headers: {
				"Content-Type": "application/json",
			},
		})
			.then((data) => data.json())
			.then((json: Flowtime) => {
				// eslint-disable-next-line no-console
				setFlowtime(json);
			})
			.catch((err) => {
				console.error(err);
			});
	}

	function stopWorking() {
		const stopFlow: StopFlowtimeRequest = {
			startTime: flowtime.startTime,
			task: flowtime.task,
			id: flowtime.id,
			stopTime: new Date().toISOString(),
			interruptions: flowtime.interruptions,
		};
		void fetch("api/flowtime", {
			method: "PUT", // or 'PUT'
			body: JSON.stringify(stopFlow), // data can be `string` or {object}!
			headers: {
				"Content-Type": "application/json",
			},
		})
			.then((data) => data.json())
			.then((_: Flowtime) => {
				// eslint-disable-next-line no-console
				setFlowtime(emptyFlowtime);
			})
			.catch((err) => {
				console.error(err);
			});
	}

	function addInterruption() {
		setFlowtime({
			...flowtime,
			interruptions: flowtime.interruptions + 1,
		});
	}

	function handleTaskChange(event: React.FormEvent<HTMLInputElement>) {
		setFlowtime({ ...flowtime, task: event.currentTarget.value });
		event.preventDefault();
	}

	function isInvalid(): boolean {
		if (flowtime.task === "") {
			return true;
		}

		return false;
	}

	function isNotStoppable(): boolean {
		if (isInvalid()) {
			return true;
		}

		return flowtime.startTime === "";
	}

	let currentFlowtimeSection = <div></div>;
	if (!isNotStoppable()) {
		currentFlowtimeSection = (
			<div>
				<h2>Current flowtime</h2>
				task: {flowtime.task} <br />
				startTime: {flowtime.startTime} <br />
				stopTime: {flowtime.stopTime} <br />
				interruptions: {flowtime.interruptions} <br />
			</div>
		);
	}

	return (
		<main>
			<form onSubmit={startWorking}>
				<label>
					Task:
					<input type="text" value={flowtime.task} onChange={handleTaskChange} />
				</label>
				<input type="submit" value="Start" disabled={isInvalid() || !isNotStoppable()} />
			</form>
			<button onClick={stopWorking} disabled={isNotStoppable()}>
				Stop
			</button>
			<button onClick={addInterruption} disabled={isNotStoppable()}>
				Interruption
			</button>
			{currentFlowtimeSection}
		</main>
	);
}
