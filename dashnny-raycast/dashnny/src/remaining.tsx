import { Detail, getPreferenceValues } from "@raycast/api";
import fetch from "node-fetch";
import { useEffect, useState } from "react";

type Pomodoro = {
  endTime: string;
};


export default function Command() {
  const [activePomodoro, setActivePomodoro] = useState<Pomodoro>({endTime: ""});

	useEffect(() => {
		fetch(`${getPreferenceValues().dashnnyApiUrl}/Pomodoros?isActive=true`, {
			method: "GET",
			headers: {
				"X-Space-App-Key": getPreferenceValues().dashnnyApiKey,
				"Content-Type": 'application/json'
			}
		})
		.then(response => response.json())
		.then((pomodorosParam) => {
			const pomodoros = pomodorosParam as Pomodoro[]
			if (pomodoros.length > 0) {
				setActivePomodoro(pomodoros[0]);
			}
		})
	}, []);

  return (
		<Detail markdown={ `${activePomodoro?.endTime ?? "No pomodoro active"}` }></Detail>
  );
}
