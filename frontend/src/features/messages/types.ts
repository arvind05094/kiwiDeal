export interface StartConversationRequest {
  listingId: string;
  listingTitle: string;
  recipientId: string;
  recipientName: string;
  initialMessage: string;
}

export interface SendMessageRequest {
  content: string;
}
