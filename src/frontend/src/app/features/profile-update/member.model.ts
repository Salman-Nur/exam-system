export interface Member {
  Id: string | null;
  FullName: string | null;
  Email: string | null;
  ProfilePicture: File | undefined | null;
  ProfilePictureUrl?: string | null;
}
