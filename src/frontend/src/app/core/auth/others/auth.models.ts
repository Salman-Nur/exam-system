export interface User {
  id: string | null;
  name: string | null;
  email: string | null;
  profilePicture: string | null;
  member: boolean;
  internalUser: boolean;
  manageMemberClaim: boolean;
  manageMember: boolean;
  manageQuestion: boolean;
  questionCreate: boolean;
  questionView: boolean;
  questionEdit: boolean;
  questionDelete: boolean;
  manageExam: boolean;
  examCreate: boolean;
  examView: boolean;
  examEdit: boolean;
  examDelete: boolean;
  manageLog: boolean;
}

export interface AuthClaim {
  type: string;
  value: string;
}
