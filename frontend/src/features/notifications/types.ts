export type NotificationType =
  | "AuctionWon"
  | "AuctionLost"
  | "ItemSold"
  | "ItemPurchased"
  | "NewMessage";

export interface NotificationDto {
  id: string;
  type: NotificationType;
  title: string;
  message: string;
  linkUrl: string | null;
  isRead: boolean;
  createdAt: string;
}

export interface NotificationsResponseDto {
  items: NotificationDto[];
  unreadCount: number;
}
