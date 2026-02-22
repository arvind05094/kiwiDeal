export interface StartConversationRequest {
  recipientId: string;
  recipientName: string;
  initialMessage: string;
}
export interface SendMessageRequest {
  content: string;
}
