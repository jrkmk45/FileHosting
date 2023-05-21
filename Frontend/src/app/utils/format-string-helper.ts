import { Injectable } from "@angular/core";

const BYTES_IN_GIGABYTE = Math.pow(1024, 3);
const BYTES_IN_MEGABYTE = Math.pow(1024, 2);
const BYTES_IN_KILOBYTE = 1024;

@Injectable({
  providedIn: 'root'
})
export class FormatStringHelper {


  public convertBytesToString(bytes: number, precision: number = 2) {

    if (bytes < BYTES_IN_KILOBYTE) {
      return `${bytes} B`;
    }
  
    if (bytes < BYTES_IN_MEGABYTE) {
      var kb = (bytes / BYTES_IN_KILOBYTE).toFixed(precision);
      return `${kb} KB`;
    }
  
    if (bytes < BYTES_IN_GIGABYTE) {
      var mb = (bytes / BYTES_IN_MEGABYTE).toFixed(precision);
      return `${mb} MB`;
    }
  
    var gb = (bytes / BYTES_IN_GIGABYTE).toFixed(precision);
    return `${gb} GB`;
  
  }
}
