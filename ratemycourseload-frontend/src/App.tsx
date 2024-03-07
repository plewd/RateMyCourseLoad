import './App.css'
import SelectionPage from '@routes/SelectionPage/SelectionPage.tsx'
// import { Course } from '@types'
import coursePrefixList from '@assets/course_prefixes.json'

// const getAllCourseOptions = (): Course[] => {
//     return courseList
//         .filter((item) => 'exact' in item['credit_hour'])
//         .map((item) => ({
//             Name: item.name,
//             Title: item.title,
//             CreditHours: item['credit_hour'].exact!,
//         }))
// }

function App() {
    // fetch all valid courses
    const coursePrefixOptions: string[] = coursePrefixList.coursePrefixes

    return (
        <>
            <h1>RateMyCourseLoad</h1>
            <SelectionPage coursePrefixOptions={coursePrefixOptions} />
        </>
    )
}

export default App
