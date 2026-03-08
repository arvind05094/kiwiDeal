import { useQuery } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { formatDistanceToNow } from "date-fns";
import { MessageSquare } from "lucide-react";
import { getConversations } from "./api";
import { LoadingSpinner } from "@/shared/components/LoadingSpinner";
import { EmptyState } from "@/shared/components/EmptyState";
import { useInboxHub } from "./useInboxHub";

export function InboxPage() {
  const navigate = useNavigate();
  useInboxHub();

  const { data: conversations, isLoading } = useQuery({
    queryKey: ["conversations"],
    queryFn: getConversations,
  });

  if (isLoading) return <LoadingSpinner />;

  if (!conversations?.length) {
    return (
      <EmptyState
        icon={<MessageSquare className="h-8 w-8" />}
        description="No conversations yet"
      />
    );
  }

  return (
    <div className="max-w-2xl mx-auto">
      <h1 className="text-2xl font-semibold text-gray-900 mb-6">Inbox</h1>
      <div className="divide-y divide-gray-100 border border-gray-200 rounded-lg bg-white overflow-hidden">
        {conversations.map((conv) => (
          <button
            key={conv.id}
            onClick={() => navigate(`/messages/${conv.id}`)}
            className="w-full text-left px-5 py-4 hover:bg-gray-50 transition-colors flex items-start gap-4"
          >
            <div className="flex-1 min-w-0">
              <div className="flex items-center justify-between gap-2 mb-0.5">
                <span className="font-medium text-gray-900 truncate">
                  {conv.otherUserName}
                </span>
                <span className="text-xs text-gray-400 shrink-0">
                  {formatDistanceToNow(new Date(conv.updatedAt), {
                    addSuffix: true,
                  })}
                </span>
              </div>

              <p className="text-sm text-gray-400 truncate">
                {conv.lastMessagePreview}
              </p>
            </div>
            {conv.unreadCount > 0 && (
              <span className="shrink-0 mt-1 inline-flex items-center justify-center h-5 min-w-5 px-1.5 rounded-full bg-orange-500 text-white text-xs font-medium">
                {conv.unreadCount}
              </span>
            )}
          </button>
        ))}
      </div>
    </div>
  );
}
