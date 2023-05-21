import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IFile } from 'src/app/models/file';
import { FilesService } from 'src/app/services/files.service';
import { SnackbarService } from 'src/app/services/snackbar.service';

@Component({
  selector: 'app-control-bar',
  templateUrl: './control-bar.component.html',
  styleUrls: ['./control-bar.component.css']
})
export class ControlBarComponent {

  allChecked = false;
  @Input() files? : IFile[];
  @Input() showDeleteButton = true;

  @Output() filesDeleted = new EventEmitter<IFile[]>();

  constructor(private filesService: FilesService,
    private snackbar: SnackbarService) {

  }

  onCheckBoxClick() {
    this.files!.map(f => f.checked = this.allChecked);
  }

  onDeleteClick() {
    var checkedFiles = this.getCheckedFiles();
    if (!checkedFiles.length) {
      this.snackbar.showMessage("Виберіть файли для видалення!");
    }

    this.filesService.deleteFiles(checkedFiles).subscribe({
      next: () => {
        this.filesDeleted.emit(checkedFiles);
      },
      error: () => {
        this.snackbar.showMessage("Error occured while deleting");
      }
    });
  }


  async onDownloadFiles() {
    let checkedFiles = this.getCheckedFiles();
    if (!checkedFiles.length) {
      this.snackbar.showMessage("Виберіть файли для завантаження!");
    }

    for (let file of checkedFiles) {
      this.filesService.downloadFile(file.id);
      await this.delay(300);
    }

  }

  delay(time: number) {
      return new Promise(resolve => setTimeout(resolve, time) );
  }

  getCheckedFiles() {
    return this.files!.filter(f => f.checked);
  }
}
