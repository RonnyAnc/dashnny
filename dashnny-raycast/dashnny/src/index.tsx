import { Form, ActionPanel, Action, showToast, getPreferenceValues } from "@raycast/api";
import fetch from "node-fetch";

type Values = {
  label: string;
  channel: string;
  duration: string;
	isFromWork: boolean;
};

type StartPomodoroRequest = {
  label: string | null;
  notificationChannel: string | null;
  durationInMinutes: number;
	startTime: string;
	isFromWork: boolean;
	numberInCycle: number;
};

export default function Command() {
  async function handleSubmit(values: Values) {
		const startPomodoroRequest: StartPomodoroRequest = {
				label: values.label || null,
				notificationChannel: values.channel || null,
				durationInMinutes: parseInt(values.duration, 10),
				isFromWork: values.isFromWork,
				numberInCycle: 1,
				startTime: new Date().toISOString()
			}
		const preferences = getPreferenceValues();
		const response = await fetch(`${preferences.dashnnyApiUrl}/Pomodoros`, {
			method: "POST",
			headers: {
				"X-Space-App-Key": preferences.dashnnyApiKey,
				"Content-Type": 'application/json'
			},
			body: JSON.stringify(startPomodoroRequest)
		});
		if (response.status > 400) {
			console.error(response.statusText);
			console.error(await response.text());
			return;
		}
		await response.json();
		showToast({title: "Pomodoro Started"});
  }

  return (
    <Form
      actions={
        <ActionPanel>
          <Action.SubmitForm onSubmit={handleSubmit} />
        </ActionPanel>
      }
    >
      <Form.Description text="This form allows you to start a pomodoro." />
      <Form.TextField id="label" title="Label" placeholder="Enter text" defaultValue="" />
			<Form.TextField id="duration" title="Duration" placeholder="Duration in minutes (25 default)" defaultValue="25" />
			<Form.Checkbox id="isFromWork" title="Is from work" label="Is from work?" defaultValue={false}></Form.Checkbox>
      <Form.Dropdown id="dropdown" title="Dropdown" defaultValue="">
        <Form.Dropdown.Item value="" title="No channel" />
				<Form.Dropdown.Item value="study" title="study" />
				<Form.Dropdown.Item value="development-pomodoros" title="development-pomodoros" />
      </Form.Dropdown>
    </Form>
  );
}
