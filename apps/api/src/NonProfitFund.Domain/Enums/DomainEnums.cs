namespace NonProfitFund.Domain.Enums;

public enum CampaignStatus { Draft, Published, Paused, Completed, Archived }
public enum DonationStatus { Pending, Succeeded, Failed, Refunded, Cancelled }
public enum PaymentStatus { RequiresAction, Processing, Succeeded, Failed, Refunded }
public enum RecurringPlanStatus { Active, Paused, Cancelled, Completed }
public enum NotificationChannel { Email, Sms }
public enum NotificationStatus { Queued, Sent, Failed }
public enum ReceiptStatus { Pending, Generated, Sent, Failed }

