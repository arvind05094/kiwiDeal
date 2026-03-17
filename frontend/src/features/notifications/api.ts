import client from "@/shared/api/client";
import type { NotificationsResponseDto } from "./types";

export const getNotifications = async (): Promise<NotificationsResponseDto> => {
  const res = await client.get<NotificationsResponseDto>("/notifications");
  return res.data;
};

export const markNotificationAsRead = async (id: string): Promise<void> => {
  await client.post(`/notifications/${id}/read`);
};

export const markAllNotificationsAsRead = async (): Promise<void> => {
  await client.post("/notifications/read-all");
};
