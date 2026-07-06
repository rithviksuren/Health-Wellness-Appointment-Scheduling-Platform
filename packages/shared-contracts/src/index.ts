export type ProblemDetails = {
  type?: string;
  title: string;
  status: number;
  detail?: string;
  instance?: string;
  errors?: Record<string, string[]>;
};

export type ApiEnvelope<T> = {
  data: T;
  correlationId: string;
};

export type Role = "Donor" | "Volunteer" | "Treasurer" | "Campaign Manager" | "Admin";

