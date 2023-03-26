import { Pipe, PipeTransform } from "@angular/core";
@Pipe({
    name: 'updateRows'
})
export class UpdateRowsPipe implements PipeTransform {
    transform<T>(value: T[], perRow: number): T[][] {
        let rows: T[][] = [];
        for (let i = 0; i < value.length; i += perRow) {
            rows.push(value.slice(i, i + perRow))
        }
        return rows;
    }
}