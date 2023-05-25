import { Component, ElementRef, EventEmitter, Output, ViewChild } from '@angular/core';
import { FilesService } from 'src/app/services/files.service';
import { SnackbarService } from 'src/app/services/snackbar.service';
import { IFile } from 'src/app/models/file';
import { IUploadingFile } from 'src/app/models/uploading-file';


@Component({
  selector: 'app-upload-file',
  templateUrl: './upload-file.component.html',
  styleUrls: ['./upload-file.component.css']
})
export class UploadFileComponent {
  @ViewChild('fileInput') fileInput!: ElementRef;

  isLoading = false;
  selectedFiles: IUploadingFile[] = [];

  @Output() fileUploaded = new EventEmitter<IFile>();

  constructor(private filesService: FilesService,
    private snackbar: SnackbarService) {}

  onFileSelect(event : Event): void {
    let files: FileList = (event.target as HTMLInputElement).files!;

    for (let i = 0; i < files.length; i++) {
      let file = files.item(i);
      if (file) {
        this.selectedFiles.unshift({ file: file });
        let reader = new FileReader();
        reader.onload = (e) => {
          const contents = e.target!.result;

        };
        reader.readAsText(file);
      }
    }
    this.fileInput.nativeElement.value = '';
  }

  onBrowseFilesClick() {
    this.fileInput.nativeElement.click();
  }

  onFilesDrop(event: DragEvent) {
    event.preventDefault();
    let files = event.dataTransfer?.files;
    for (let i=0; i<files!.length; i++) {
      this.selectedFiles.unshift({ file: files!.item(i)! });
    }
  }

  onDragOver(event: any) {
    event.preventDefault();
  }

  onUploadFiles() {
    for (let selectedFile of this.selectedFiles) {
      const formData = new FormData();

      formData.append('file', selectedFile.file);
      
      if (selectedFile.isPrivate == null) {
        selectedFile.isPrivate = false;
      }
      this.filesService.uploadFile(formData, selectedFile.isPrivate!).subscribe({
        next: (response) => {
          if (response != null) {
            if (response.progress != 'finished') {
              selectedFile.progress = response.progress;
              selectedFile.loadedBytes = response.loadedBytes;
              selectedFile.totalLoadrequirement = response.totalLoadrequirement;
            }
            else {
              selectedFile.file = response.result;
              this.fileUploaded.emit(response.result);
            }
          }
        },
        error: () => {
          this.snackbar.showMessage(`Error occured while uploading file`);
        }
      })
      formData.delete('file');
    }
  }

  onCancelAllClick() {
    this.selectedFiles = [];
  }
}
