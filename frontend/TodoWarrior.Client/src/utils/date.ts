export function toDateOnlyInput(date?: string | null): string {
  // expect date like ISO or undefined -> YYYY-MM-DD
  if (!date) return new Date().toISOString().slice(0,10);
  return new Date(date).toISOString().slice(0,10);
}

export function toDateTimeLocalInput(date?: string|null): string {
  if (!date) return "";
  const d = new Date(date);
  const pad=(n:number)=>String(n).padStart(2,"0");
  return `${d.getFullYear()}-${pad(d.getMonth()+1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
}

export function fromDateTimeLocalInput(v: string): string | null {
  // returns ISO string (local interpreted as local time)
  if (!v) return null;
  const d = new Date(v);
  return d.toISOString();
}