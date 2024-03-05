import * as O from 'fp-ts/Option'

export type Course = { Name: string; Title: string; CreditHours: number }
export type SelectedCourse = O.Option<Course>
