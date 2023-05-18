import { Detail, getPreferenceValues } from "@raycast/api";
import { DateTime } from "luxon";
import fetch from "node-fetch";
import { useEffect, useState } from "react";

type Pomodoro = {
	label: string;
	id: string;
  endTime: string;
};

type ActivePomodoro = {
	remainingTime: string;
	label: string;
	id: string;
} | null | "unitialized";

export default function Command() {
  const [activePomodoro, setActivePomodoro] = useState<ActivePomodoro>("unitialized");

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
				const pomodoro = pomodoros[0];
				const diff = DateTime.fromISO(pomodoro.endTime).diff(DateTime.now(), ['hours', 'minutes', 'seconds']);
				const formattedDiff = `${diff.hours.toString().padStart(2, '0')}:${diff.minutes.toString().padStart(2, '0')}:${Math.floor(diff.seconds).toString().padStart(2, '0')}`;
				setActivePomodoro({
					id: pomodoro.id,
					label: pomodoro.label,
					remainingTime: formattedDiff
				});
				return;
			}
			setActivePomodoro(null);
		})
	}, []);

  return (
		<>
		{ activePomodoro == null ? 
			(<Detail markdown="No timer active"></Detail>) :
			( activePomodoro == "unitialized" ? 
				<Detail markdown="Loading..."></Detail> :
				<Detail markdown={`
## Working on [${activePomodoro.label}]
Remaining time: ${activePomodoro.remainingTime}

Pomodoro id: ${activePomodoro.id}
`}></Detail>)
		}
		</>
  );
}
