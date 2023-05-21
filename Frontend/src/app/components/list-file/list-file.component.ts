import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { IFile } from 'src/app/models/file';
import { FilesService } from 'src/app/services/files.service';
import { FormatStringHelper } from 'src/app/utils/format-string-helper'

@Component({
  selector: 'app-list-file',
  templateUrl: './list-file.component.html',
  styleUrls: ['./list-file.component.css']
})
export class ListFileComponent {

  
  @Output() public selectionUpdated = new EventEmitter<string>();

  @Input() public file?: IFile;


  showFullTitlePopup = false;
  pupupTopPosition?: number;

  constructor(private filesService: FilesService,
    public formatHelper: FormatStringHelper) {}

  onDownloadClicked() {
    this.filesService.downloadFile(this.file!.id);
  }
  
  showTitlePopup(event : MouseEvent) {
    this.showFullTitlePopup = true;

    const element = event.target as HTMLElement;
    const rect = element.getBoundingClientRect();
    this.pupupTopPosition = rect.top+19;
  }

  onCheckBoxClick() {
    this.selectionUpdated.emit(this.file!.id);
  }
}
