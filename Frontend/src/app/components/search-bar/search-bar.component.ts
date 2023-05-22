import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { IFile } from 'src/app/models/file';
import { FilesService } from 'src/app/services/files.service';
import { SnackbarService } from 'src/app/services/snackbar.service';

@Component({
  selector: 'app-search-bar',
  templateUrl: './search-bar.component.html',
  styleUrls: ['./search-bar.component.css']
})
export class SearchBarComponent {

  @Output() filesStateChanged = new EventEmitter<IFile[]>();

  searchTerm?: string

  constructor(private filesService: FilesService,
    private snackbar: SnackbarService) {}


  onInputChange(event : any) {
    event = event.trim();
    if (event == '') {
      this.getAllFiles();
      return;
    }

    this.filesService.searchFiles(event).subscribe({
      next: (files) => {
        this.filesStateChanged.emit(files);
      },
      error: () => {
        this.snackbar.showMessage("Error occured while loading files");
      }
    })
  }

  getAllFiles() {
    this.filesService.getAllFiles().subscribe({
      next: (files) => {
        this.filesStateChanged.emit(files);
      },
      error: () => {
        this.snackbar.showMessage("Error occured while loading files");
      }
    })
  }
}
