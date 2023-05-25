import { IUser } from "./users";

export interface IFile {
  id: string;
  fullName: string;
  name: string;
  extension: string;
  size: number;
  createdDate: Date;

  owner?: IUser;
  accessability? : number;
  permittedUsers? : IUser[];

  checked?: boolean;
  uploadProgress? : number;
}