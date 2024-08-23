import { Component, DestroyRef, TemplateRef, ViewChild, computed, effect, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ReactiveFormsModule } from '@angular/forms';
import { NgbModal, NgbToastModule } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { Sorter, ColumnDefinition, CellComponent, EmptyCallback, ValueBooleanCallback, ValueVoidCallback } from 'tabulator-tables';
import { environment } from '../../../environments/environment.development';
import { commonToastMessages } from '../../shared/others/misc/common-toast-messages';
import { ReactiveFormUtilsService } from '../../shared/others/services/reactive-form-utils.service';
import { ErrorLogModel } from './error-log.model';
import { ErrorLogService } from './error-log.service';
import { NgIf, NgTemplateOutlet, NgClass, DatePipe } from '@angular/common';
import { NgxBootstrapInputDirective } from '../../shared/others/directives/ngx-bootstrap-input.directive';
import flatpickr from 'flatpickr';
import { firstValueFrom } from 'rxjs';
import { TabulatorBasicComponent } from '../../shared/tabulator/component/tabulator-basic.component';
import { TabulatorDataService } from '../../shared/tabulator/others/tabulator-data.service';

declare global {
  interface Window {
    view: (event: MouseEvent, id: number) => void;
  }
}

@Component({
  selector: 'app-error-log',
  standalone: true,
  imports: [
    TabulatorBasicComponent,
    ReactiveFormsModule,
    NgxBootstrapInputDirective,
    NgIf,
    NgbToastModule,
    NgTemplateOutlet,
    NgClass
  ],
  providers: [TabulatorDataService, DatePipe],
  templateUrl: './error-log.component.html',
  styleUrl: './error-log.component.scss'
})

export class ErrorLogComponent {
  readonly tabulatorDataService = inject(TabulatorDataService);
  readonly reactiveFormUtils = inject(ReactiveFormUtilsService);
  private readonly modalService = inject(NgbModal);
  private readonly toastr = inject(ToastrService);
  private readonly ErrorLogService = inject(ErrorLogService);
  readonly $isAjaxRequestOutgoing = signal(false);
  readonly $isAjaxDataIncoming = signal(true);

  readonly $$isAnyAjaxOperationRunning = computed(() => {
    return this.$isAjaxDataIncoming() || this.$isAjaxRequestOutgoing();
  });

  $selectedCellData = signal<Array<ErrorLogModel>>([]);
  readonly $$isSelectedCellDataEmpty = computed(() => {
    return this.$selectedCellData().length === 0;
  });
  readonly $$canUpdateCellData = computed(() => {
    return this.$selectedCellData().length === 1;
  });

  private readonly destroyRef = inject(DestroyRef);

  @ViewChild("deleteModal")
  deleteModal: TemplateRef<any> | undefined;

  readonly environment = environment;

  initialSortRules: Array<Sorter> = [{ column: "time", dir: "desc" }];

  columns: ColumnDefinition[] = [
    {
      title: "Message",
      field: "message",
      sorter: "string",
      width: 400,
      headerFilterFunc: "like",
      headerFilter: "input"
    },
    {
      title: "Time",
      field: "time",
      width: 300,
      headerFilterPlaceholder: "Enter date",
      sorter: "datetime",
      sorterParams: { format: "DD/MM/YYYY" },
      formatter: (cell: CellComponent) => {
        const dateValue = cell.getValue();
        const formattedDate = this.datePipe.transform(dateValue, 'shortDate');
        return formattedDate ? formattedDate : ''; // Ensure it always returns a string
      },
      headerFilter: this.singleDateEditor,
      headerFilterFunc: "="
    },
    {
      title: "Log Level",
      field: "levelName",
      sorter: "string",
      width: 300,
      formatter: this.levelSetter,
      headerFilterFunc: "=",
      headerFilter: "list",
      headerFilterParams: {
        values: {
          Trace: "Trace",
          Debug: "Debug",
          Information: "Information",
          Warning: "Warning",
          Error: "Error",
          Critical: "Critical",
          "": "All",
        },
      }
    },
    {
      title: "Action",
      field: "id",
      width: 300,
      headerSort: false,
      headerHozAlign: "center",
      hozAlign: "center",
      formatter: function (cell) {
        var id = cell.getValue();
        return `<a class='btn btn-primary mr-3' href='javascript:void(0)' onclick='view(event, ${id})'>View</a>`;
      }
    }
  ];

  constructor() {
    window.view = this.view.bind(this);
  }

  message: string = '';
  levelName: string = '';
  time: string | null = null;
  datePipe = inject(DatePipe);
  @ViewChild("viewModal")
  viewModal: TemplateRef<any> | undefined;

  async view(event: MouseEvent, id: number) {
    event.stopPropagation();
    if (!event || !id || !this.viewModal) {
      return;
    }
    try {
      const res = await firstValueFrom(this.ErrorLogService.getById(id));
      this.message = res.message;
      this.levelName = res.levelName.toLocaleLowerCase();
      if (res.time != null) {
        this.time = this.datePipe.transform(res.time, 'h:mm a EEEE, MMMM d, yyyy');
      } else {
        this.time = '';
      }
      const result = await this.openModalView(this.viewModal);
    } catch (error) {
      this.toastr.error('Failed to view the log');
    }
  }

  levelSetter(cell: CellComponent) {
    var value = cell.getValue();
    switch (value) {
      case "Trace":
        return `<span class="badge bg-blue" style="color: white;">${value}</span>`;
      case "Debug":
        return `<span class="badge bg-green" style="color: white;">${value}</span>`;
      case "Information":
        return `<span class="badge bg-azure" style="color: white;">${value}</span>`;
      case "Warning":
        return `<span class="badge bg-yellow" style="color: white;">${value}</span>`;
      case "Error":
        return `<span class="badge bg-red" style="color: white;">${value}</span>`;
      case "Critical":
        return `<span class="badge bg-purple" style="color: white;">${value}</span>`;
      default:
        return value;
    }
  }

  singleDateEditor(
    cell: CellComponent,
    onRendered: EmptyCallback,
    success: ValueBooleanCallback,
    cancel: ValueVoidCallback,
    editorParams: {}
  ) {
    const editor = document.createElement("input");
    editor.setAttribute("type", "text");
    editor.style.cssText = "width:100%; height:100%;";

    onRendered(function () {
      flatpickr(editor, {
        enableTime: false,
        dateFormat: "d/m/Y",
        onClose: function (selectedDates, dateStr, instance) {
          if (selectedDates.length) {
            success(dateStr);
          } else {
            cancel(null);
          }
        }
      });
      editor.focus();
    });
    return editor;
  }

  onAjaxDataIncoming(event: boolean) {
    this.$isAjaxDataIncoming.set(event);
  }

  onSelectedCellData(event: any[]) {
    if (event === null) {
      this.$selectedCellData.set([]);
      return;
    }

    const allPropertiesExists = event.every((elem) => {
      return elem?.id && elem?.message && elem?.levelName && elem?.time;
    });

    if (allPropertiesExists) {
      this.$selectedCellData.set(event as ErrorLogModel[]);
    }
  }

  ngOnInit() {
    this.tabulatorDataService.isAjaxRequestOutgoing$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (val) => {
          this.$isAjaxRequestOutgoing.set(val);
        }
      });
  }

  isOpen: any;

  async openModalView(content: TemplateRef<any>): Promise<boolean> {
    try {
      this.isOpen = true;
      return await this.modalService.open(content, { centered: false }).result;
    } catch (err) {
      return false;
    }
  }

  async openModal(content: TemplateRef<any>): Promise<boolean> {
    try {
      return await this.modalService.open(content, { centered: true }).result;
    } catch (err) {
      return false;
    }
  }

  async delete() {
    console.log(this.$selectedCellData());
    if (this.$$isSelectedCellDataEmpty() || !this.deleteModal) {
      return;
    }
    const data = this.$selectedCellData();

    const result = await this.openModal(this.deleteModal);
    if (result) {
      this.tabulatorDataService.outgoingAjaxRequestInProgress();
      if (data.length == 1) {
        this.ErrorLogService.delete(data[0].id)
          .subscribe({
            error: () => {
              this.toastr.error(commonToastMessages.DeletedFailed);
              this.done();
            },
            next: () => {
              this.tabulatorDataService.reloadTableData();
              this.toastr.success(commonToastMessages.Deleted);
              this.done();
            }
          });
      }
      else {
        this.ErrorLogService.deleteMany(data.map((val) => val.id))
          .subscribe({
            error: () => {
              this.toastr.error(commonToastMessages.DeletedFailed);
              this.done();
            },
            next: () => {
              this.tabulatorDataService.reloadTableData();
              this.toastr.success(commonToastMessages.Deleted);
              this.done();
            }
          });
      }
    }
  }

  private done(clearSelectedCellData: boolean = true) {
    if (clearSelectedCellData) {
      this.$selectedCellData.set([]);
    }
    this.tabulatorDataService.outgoingAjaxRequestDone();
  }
}
