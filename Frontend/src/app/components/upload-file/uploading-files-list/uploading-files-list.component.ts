import { Component, Input, OnInit, Output } from '@angular/core';
import { IUploadingFile } from 'src/app/models/uploading-file';
import { FormatStringHelper } from 'src/app/utils/format-string-helper';

@Component({
  selector: 'app-uploading-files-list',
  templateUrl: './uploading-files-list.component.html',
  styleUrls: ['./uploading-files-list.component.css']
})
export class UploadingFilesListComponent implements OnInit {
  @Input() uploadingFiles?: IUploadingFile[];

  constructor(public formatHelper: FormatStringHelper) {}

  ngOnInit() {
    
  }
  
  onRemoveFileClick(index : number) {
    this.uploadingFiles!.splice(index, 1);
  }

  onPrivateFileCheckBoxClick(i : number) {
    this.uploadingFiles![i].isPrivate = !this.uploadingFiles![i].isPrivate;
  }
}