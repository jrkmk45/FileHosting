import { Injectable } from '@angular/core';
import { HttpClient, HttpEventType, HttpHeaders, HttpParams, HttpResponse } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { API_URL } from '../constants';
import { IFile } from '../models/file';
import { IUser } from '../models/users';

@Injectable({
  providedIn: 'root'
})
export class FilesService {

  constructor(private http: HttpClient) { }

  uploadFile(inputData: FormData, isPrivate: boolean) : Observable<any> {
    let headers = new HttpHeaders({
      'IsPrivate': isPrivate.toString(),
    });

    return this.http.post(API_URL + 'files', inputData, {
      headers: headers,
      reportProgress: true,
      observe: 'events',
      responseType: 'json',
    },).pipe(
      map(event => {
        if (event.type === HttpEventType.UploadProgress) {
          const percentDone = Math.round((100 * event.loaded) / event.total!);
          return { 
            progress: percentDone,
            loadedBytes: event.loaded,
            total: event.total
          };
        }
        else if (event instanceof HttpResponse) {
          return {  progress: "finished", result: event.body };
        }
        else {
          return null;
        }
      })
    );
  }

  downloadFile(fileId: string) {
    this.http.get(`${API_URL}files/${fileId}/download-link`, {responseType: 'text'}).subscribe((url: string) => {
      let downloadLink = document.createElement('a');
      downloadLink.href = url;
      downloadLink.click();
      window.URL.revokeObjectURL(url);
    });
    // (url: string) => {
    //   let downloadLink = document.createElement('a');
    //   downloadLink.href = url;
    //   downloadLink.click();
    //   window.URL.revokeObjectURL(url);
    // }
    // let url = `${API_URL}files/${fileId}/download`;



  } 

  getFileMetadata(fileId: string) : Observable<IFile> {
    return this.http.get<IFile>(`${API_URL}files/${fileId}`);
  }

  getUserFilesMetadata(userId: number) : Observable<IFile[]> {
    return this.http.get<IFile[]>(`${API_URL}users/${userId}/files`);
  }

  getAllFiles() : Observable<IFile[]> {
    return this.http.get<IFile[]>(`${API_URL}files`);
  }

  searchFiles(searchTerm: string) : Observable<IFile[]> {
    return this.http.get<IFile[]>(`${API_URL}files/search?name=${searchTerm}`);
  }

  deleteFiles(files: IFile[]) {
    let fileIds = files.map(f => f.id);
    let httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
      }),
      body: fileIds,
    };
    return this.http.delete(`${API_URL}files`, httpOptions);
  }

  patchFile(id: string, body : any) {
    return this.http.patch(`${API_URL}files/${id}`, body);
  }

  queryUserFiles(userId: number, access?: number, search?: string) : Observable<IFile[]> {
    let httpParams = new HttpParams();
    if (access != null) {
      httpParams = httpParams.set('access', access);
    }

    if (search != null) {
      httpParams = httpParams.set('search', search);
    }

    return this.http.get<IFile[]>(`${API_URL}users/${userId}/files/`, {params: httpParams});
  }
}
