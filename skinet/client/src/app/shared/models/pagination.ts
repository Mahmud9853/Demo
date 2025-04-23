export interface Pagination<T> {
    pageIndex: number;
    pageSize: number;
    count: number;
    product: T;
}