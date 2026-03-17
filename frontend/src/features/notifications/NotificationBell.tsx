import { Bell } from "lucide-react";
import { useNavigate } from "react-router-dom";
import { useQuery, useQueryClient, useMutation } from "@tanstack/react-query";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import {
  getNotifications,
  markNotificationAsRead,
  markAllNotificationsAsRead,
} from "./api";
import { useNotificationsHub } from "./useNotificationsHub";
import type { NotificationDto } from "./types";

function timeAgo(dateString: string): string {
  const diffMs = Date.now() - new Date(dateString).getTime();
  const minutes = Math.floor(diffMs / 60000);
  if (minutes < 1) return "just now";
  if (minutes < 60) return `${minutes}m ago`;
  const hours = Math.floor(minutes / 60);
  if (hours < 24) return `${hours}h ago`;
  const days = Math.floor(hours / 24);
  return `${days}d ago`;
}

export function NotificationBell() {
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  useNotificationsHub();

  const { data } = useQuery({
    queryKey: ["notifications"],
    queryFn: getNotifications,
    refetchInterval: 60000,
  });

  const markReadMutation = useMutation({
    mutationFn: markNotificationAsRead,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["notifications"] }),
  });

  const markAllReadMutation = useMutation({
    mutationFn: markAllNotificationsAsRead,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["notifications"] }),
  });

  const notifications = data?.items ?? [];
  const unreadCount = data?.unreadCount ?? 0;

  const handleClick = (notification: NotificationDto) => {
    if (!notification.isRead) {
      markReadMutation.mutate(notification.id);
    }
    if (notification.linkUrl) {
      navigate(notification.linkUrl);
    }
  };

  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <button className="relative flex items-center justify-center rounded-lg p-2 text-gray-600 hover:bg-gray-50 hover:text-gray-900 transition-colors">
          <Bell className="h-5 w-5" />
          {unreadCount > 0 && (
            <span className="absolute -top-1 -right-1 flex h-4.5 min-w-4.5 items-center justify-center rounded-full bg-orange-500 px-1 text-[10px] font-semibold text-white">
              {unreadCount > 9 ? "9+" : unreadCount}
            </span>
          )}
        </button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end" className="w-80">
        <div className="flex items-center justify-between px-3 py-2">
          <span className="text-sm font-semibold text-gray-900">Notifications</span>
          {unreadCount > 0 && (
            <button
              onClick={() => markAllReadMutation.mutate()}
              className="text-xs text-orange-500 hover:underline"
            >
              Mark all as read
            </button>
          )}
        </div>
        <div className="max-h-96 overflow-y-auto border-t border-gray-100">
          {notifications.length === 0 ? (
            <div className="px-3 py-6 text-center text-sm text-gray-500">
              No notifications yet
            </div>
          ) : (
            notifications.map((n) => (
              <button
                key={n.id}
                onClick={() => handleClick(n)}
                className={`flex w-full flex-col gap-0.5 px-3 py-2.5 text-left text-sm transition-colors hover:bg-gray-50 ${
                  !n.isRead ? "bg-orange-50" : ""
                }`}
              >
                <div className="flex items-center justify-between gap-2">
                  <span className="font-medium text-gray-900">{n.title}</span>
                  {!n.isRead && (
                    <span className="h-2 w-2 flex-shrink-0 rounded-full bg-orange-500" />
                  )}
                </div>
                <span className="text-gray-600 line-clamp-2">{n.message}</span>
                <span className="text-xs text-gray-400">{timeAgo(n.createdAt)}</span>
              </button>
            ))
          )}
        </div>
      </DropdownMenuContent>
    </DropdownMenu>
  );
}
