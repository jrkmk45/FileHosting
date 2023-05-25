export interface IUploadingFile {
  progress?: number;
  loadedBytes?: number;
  totalLoadrequirement?: number;
  isPrivate?: boolean;
  file: File;
}