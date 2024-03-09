import './App.css'
import SelectionPage from '@routes/SelectionPage/SelectionPage.tsx'
import coursePrefixList from '@assets/course_prefixes.json'
import SuggestionPage from '@routes/SuggestionPage/SuggestionPage.tsx'
import React from 'react'
import { Course } from '@types'
import logo from '@assets/ratemycourseload-nobackground.svg'

const year = new Date().getFullYear()

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
            <img
                style={{
                    height: '80px',
                }}
                alt="A degree scroll, the logo for the website"
                src={logo}
            />
            <h1>RateMyCourseLoad</h1>
            <SelectionPage
                coursePrefixOptions={coursePrefixOptions}
                onSubmit={onCoursesSelected}
            />
            {coursesToRate !== null && (
                <SuggestionPage
                    coursesToRate={coursesToRate}
                    onClose={() => setCoursesToRate(null)}
                />
            )}
            <footer>© {year} plewd. All rights reserved.</footer>
        </>
    )
}

export default App
