import { AfterViewInit, Component, ElementRef, HostListener, Input, OnInit, Renderer2, ViewChild } from '@angular/core';
import { hexToCSSFilter } from 'hex-to-css-filter';

@Component({
  selector: 'app-file-icon',
  templateUrl: './file-icon.component.html',
  styleUrls: ['./file-icon.component.css']
})
export class FileIconComponent {

  filterValue?: string;

  @Input() extension! : string;
  @Input() showExtension = true;
  
  backgroundColorFilter? : string;


  ngOnInit() {
    let color;
    switch (this.extension) {
      case '.exe':
        color = '#0e63ab';
        break;
      case '.pptx':
        color = '#ac3a1f';
        break;
      case '.docx':
      case '.doc':
        color = '#2980b9';
        break;
      case '.pdf':
        color = '#db1a1c';
        break;
      case '.zip':
      case '.rar':
      case '.7z':
        color = '#920159';
        break;
      case '.js':
      case '.json':
      case '.ts':
        color = '#F0DB4F';
        break;
      default:
        color = '#303030';

    }
    this.backgroundColorFilter = hexToCSSFilter(color).filter.slice(0, -1);
  }
} 
