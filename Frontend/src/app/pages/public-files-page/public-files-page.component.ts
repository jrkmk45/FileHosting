import { Component, OnInit } from '@angular/core';
import { IFile } from 'src/app/models/file';
import { FilesService } from 'src/app/services/files.service';
import { SnackbarService } from 'src/app/services/snackbar.service';

@Component({
  selector: 'app-public-files-page',
  templateUrl: './public-files-page.component.html',
  styleUrls: ['./public-files-page.component.css']
})
export class PublicFilesPageComponent implements OnInit {

  files?: IFile[]
  searchTerm?: string
  isLoading = true;

  constructor(private filesService: FilesService,
    private snackbar: SnackbarService) {}

  ngOnInit(): void {
    this.filesService.getAllFiles().subscribe({
      next: (files) => {
        this.files = files;
        this.isLoading = false;
      },
      error: () => {
        this.snackbar.showMessage("Error occured while loading files");
      }
    })
  }

  onSearchPerformed(event: IFile[]) {
    this.files = event;
  }
}
