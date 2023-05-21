export interface IFile {
  id: string
  fullName: string
  name: string
  extension: string
  size: number
  createdDate: string

  checked?: boolean
  uploadProgress? : number
}