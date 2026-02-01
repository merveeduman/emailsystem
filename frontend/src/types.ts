export type InboxMessage = {
  messageId: string;
  sender: string;
  subject: string;
  body: string;
  receivedAtUtc: string;
  integrityValid: boolean;
  signatureValid: boolean;
};