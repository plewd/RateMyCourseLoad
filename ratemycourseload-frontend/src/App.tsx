import './App.css'
import SelectionPage from '@routes/SelectionPage/SelectionPage.tsx'
import { Course } from '@types'
import courseList from '@assets/courses.json'

const getAllCourseOptions = (): Course[] => {
    return courseList
        .filter((item) => 'exact' in item['credit_hour'])
        .map((item) => ({
            Name: item.name,
            Title: item.title,
            CreditHours: item['credit_hour'].exact!,
        }))
}

function App() {
    // fetch all valid courses
    const courseOptions: Course[] = getAllCourseOptions()

    return (
        <>
            <h1>RateMyCourseLoad</h1>
            <SelectionPage courseOptions={courseOptions} />
        </>
    )
}

export default App
