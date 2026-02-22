import client from "@/shared/api/client";
import type { ConversationDto, MessageDto } from "@/shared/types/common";
import type { StartConversationRequest, SendMessageRequest } from "./types";
export const startConversation = async (
  body: StartConversationRequest,
): Promise<string> => {
  const res = await client.post("/messages/conversations", body);
  return typeof res.data === "string"
    ? res.data
    : (res.data.value ?? res.data.id ?? String(res.data));
};
export const getConversations = async (): Promise<ConversationDto[]> => {
  const res = await client.get<ConversationDto[]>("/messages/conversations");
  return res.data;
};

export const getMessages = async (
  conversationId: string,
): Promise<MessageDto[]> => {
  const res = await client.get<MessageDto[]>(
    `/messages/conversations/${conversationId}`,
  );
  return res.data;
};

export const sendMessage = async (
  conversationId: string,
  body: SendMessageRequest,
): Promise<MessageDto> => {
  const res = await client.post<MessageDto>(
    `/messages/conversations/${conversationId}/messages`,
    body,
  );
  return res.data;
};

export const markAsRead = async (conversationId: string): Promise<void> => {
  await client.post(`/messages/conversations/${conversationId}/read`);
};
