import { Course } from '@types'
import SelectedCourseList from '@components/SelectedCourseList.tsx'

interface Props {
    selectedCourses: Course[]
}

export default function SuggestionPage({ selectedCourses }: Props) {
    return (
        <>
            <SelectedCourseList
                selectedCourses={selectedCourses}
                showDelete={false}
                removeCourse={() => {}}
            />
        </>
    )
}
