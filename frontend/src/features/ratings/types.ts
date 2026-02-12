export interface RatingPromptDto {
  id: string;
  ratedUserId: string;
  ratedUserName: string;
  paymentId: string;
  role: "Buyer" | "Seller";
}

export interface SubmitRatingRequest {
  stars: number;
  comment?: string;
}
