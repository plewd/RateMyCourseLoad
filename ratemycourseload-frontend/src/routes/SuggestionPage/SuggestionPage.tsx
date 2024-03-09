import { Course } from '@types'
import { API_ENDPOINT } from '@configuration'
import {
    CircularProgress,
    Grid,
    Modal,
    ModalClose,
    ModalDialog,
    Typography,
} from '@mui/joy'
import { Rating } from '@mui/material'
import React, { useEffect } from 'react'

interface Props {
    coursesToRate: Course[]
    onClose: () => void
}

const loadingMessages = [
    '🎓 Crunching numbers and brewing the perfect course load...',
    '📚 Sorting through the library of knowledge...',
    '🔬 Mixing academic potions for your perfect semester...',
    '🧠 Feeding the hamsters that power our course load calculator...',
    '📖 Turning pages and scanning chapters for your ideal courses...',
    '🕹️ Playing Tetris with your semester schedule...',
    '🚀 Launching your academic journey to the stars...',
    '🌐 Navigating the academic web to find your perfect fit...',
    '🏋️‍♂️ Pumping intellectual iron to shape your course load...',
    "🎲 Rolling the dice on your academic future... (Just kidding, we're professionals!)",
    '🔮 Gazing into the crystal ball of academia...',
    '🎈 Inflating your potential with the perfect course load...',
    '🍀 Searching for four-leaf clovers in the academic field...',
    '🎨 Painting a masterpiece with your course selections...',
    '🕵️ On a secret mission to find the best courses for you...',
    '🗺️ Charting your course through the sea of knowledge...',
    '🍳 Cooking up a feast of knowledge for your semester...',
    '🔎 Magnifying your academic potential...',
    '🦉 Summoning the wisdom of the academic owls...',
    '🏄‍ Surfing the wave of knowledge to find your perfect courses...',
]

// Returns a number between 0..(max-1) (not inclusive of max)
const getRandomInt = (max: number) => Math.floor(Math.random() * max)

const getRandomLoadingMessage = () =>
    loadingMessages[getRandomInt(loadingMessages.length)]

const convertCourseListToStringList = (courses: Course[]) =>
    courses.map((course) => course.Name)

interface CourseRatingResponse {
    'credit hours': string
    workload: string
    balance: string
    score: string
}

export default function SuggestionPage({ coursesToRate, onClose }: Props) {
    const [open, setOpen] = React.useState<boolean>(true)
    const [isLoading, setIsLoading] = React.useState<boolean>(true)
    const [response, setResponse] = React.useState<CourseRatingResponse | null>(
        null
    )

    const fetchRating = async () => {
        try {
            const courseStrings = convertCourseListToStringList(coursesToRate)
            const rawResponse = await fetch(`${API_ENDPOINT}/course/rate`, {
                method: 'POST',
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ courses: courseStrings }),
            })
            const content = await rawResponse.json()
            console.log(content)
            setResponse(content)
            setIsLoading(false)
        } catch (error) {
            console.error('Error fetching course rating:', error)
            setIsLoading(false)
        }
    }

    useEffect(() => {
        void fetchRating()
    }, [coursesToRate])

    const scoreNum = Number(response?.['score'])
    const courseLoadSuggestions = (
        <Typography>
            <div>
                <strong>Credit Hours:</strong> {response?.['credit hours']}
            </div>
            <div>
                <strong>Workload:</strong> {response?.['workload']}
            </div>
            <div>
                <strong>Balance:</strong> {response?.['balance']}
            </div>

            <div>
                <strong>Score:</strong>{' '}
                <Rating
                    name="read-only"
                    value={scoreNum}
                    readOnly
                    style={{ verticalAlign: 'bottom' }}
                />
            </div>
        </Typography>
    )

    const loadingScreen = (
        <>
            <CircularProgress
                style={{
                    marginLeft: '20vw',
                    marginRight: '20vw',
                    marginTop: '20vh',
                    marginBottom: '10px',
                }}
            />
            <Typography style={{ marginBottom: '20vh' }}>
                {getRandomLoadingMessage()}
            </Typography>
        </>
    )

    return (
        <Modal
            open={open}
            onClose={() => {
                setOpen(false)
                onClose()
            }}
        >
            <ModalDialog color="neutral" variant="soft">
                <ModalClose />
                <Grid
                    container
                    direction="row"
                    justifyContent="center"
                    alignItems="center"
                    style={{ marginTop: '20px' }}
                >
                    {isLoading ? loadingScreen : courseLoadSuggestions}
                </Grid>
            </ModalDialog>
        </Modal>
    )
}
