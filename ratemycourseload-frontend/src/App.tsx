import './App.css'
import SelectionPage from '@routes/SelectionPage/SelectionPage.tsx'
import coursePrefixList from '@assets/course_prefixes.json'
import SuggestionPage from '@routes/SuggestionPage/SuggestionPage.tsx'
import React from 'react'
import { Course } from '@types'

export const API_ENDPOINT = process.env.REACT_APP_API_ENDPOINT

function App() {

    // fetch all valid courses
    const coursePrefixOptions: string[] = coursePrefixList.coursePrefixes
    const [coursesToRate, setCoursesToRate] = React.useState<Course[] | null>(
        null
    )
    const onCoursesSelected = (cs: Course[]) => {
        setCoursesToRate(cs)
    }

    return (
        <>
            <h1>RateMyCourseLoad</h1>
            <SelectionPage
                coursePrefixOptions={coursePrefixOptions}
                onSubmit={onCoursesSelected}
            />
            {coursesToRate !== null && (
                <SuggestionPage coursesToRate={coursesToRate} />
            )}
        </>
    )
}

export default App
